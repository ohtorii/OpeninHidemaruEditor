// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
import * as path from 'path';
import * as child_process from 'child_process';

const fs = require('fs');
const arch =require('os').arch();

let GetPath = function(args:any[]) {
    if (args && args.length > 0) {
		let path = args[0].fsPath;
		if (path){
			return path;
		}
    }
	if(vscode.window.activeTextEditor){
		return vscode.window.activeTextEditor.document.fileName;
	}
    return null;
};

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {
	context.subscriptions.push(vscode.commands.registerCommand('extension.OpenInHidemaruEditor', OpenInHidemaruEditor));
	context.subscriptions.push(vscode.commands.registerCommand('extension.OpenInHidemaruEditorEx', OpenInHidemaruEditorEx));
}

function OpenInHidemaruEditor(...args:any[]){
	const hidemaru_exe=FindHidemaruExe();
	if(! IsExistFile(hidemaru_exe)){
		vscode.window.showErrorMessage("Not found hidemaru.exe");
		return;
	}
	const filename=GetPath(args);
	if(!filename){
		return;
	}
	child_process.spawn(hidemaru_exe,[filename],{detached: true}).unref();
}

function OpenInHidemaruEditorEx(...args:any[]){
	const editor=vscode.window.activeTextEditor;
	if(! editor){
		return;
	}
	const column:number  = ConvertLineNo(editor.selection.active.character);
	const line:number    = ConvertColumnNo(editor.selection.active.line);
	const filename:string= editor.document.fileName;
	const hidemaru_exe=FindHidemaruExe();
	if(! IsExistFile(hidemaru_exe)){
		vscode.window.showErrorMessage("Not found hidemaru.exe");
		return;
	}
	child_process.spawn(hidemaru_exe,[`/j${line},${column}`, filename],{detached: true}).unref();
}

function FindHidemaruExe():string{
	const path:string = <string>vscode.workspace.getConfiguration('OpenInHidemaruEditor').get('Path');
	if(path!==""){
		return path;
	}
	if(arch==="x64"){
		const exe_x64="C:\\Program Files\\Hidemaru\\Hidemaru.exe";
		const exe_x86="C:\\Program Files (x86)\\Hidemaru\\Hidemaru.exe";
		if(IsExistFile(exe_x64)){
			return exe_x64;
		}
		return exe_x86;
	}

	//x86
	return "C:\\Program Files\\Hidemaru\\Hidemaru.exe";
}

function IsExistFile(file:string) {
	try {
		fs.statSync(file);
		return true;
	} catch(err) {
		if(err.code === 'ENOENT') {
			return false;
		}
	}
	return false;
}

function ConvertLineNo(line:number):number{
	return line+1;
}

function ConvertColumnNo(column:number):number{
	return column+1;
}

// this method is called when your extension is deactivated
export function deactivate() {}
