const path = require('path');

const { app, dialog, BrowserWindow } = require('electron');
const { CGI } = require('./scripts/cgi');

let win;

global.dotnet = cgi = new CGI("debug");

cgi.listenDisconnect(() => {
	console.log('Disconnected at ' + new Date());
	dialog.showErrorBox('Dotnet Application Disconnected', 'The Dotnet Application has disconnected from the Electron process.');
});

function createWindow() {
	win = new BrowserWindow({
		width: 800,
		height: 600,
		webPreferences: {
			nodeIntegration: true,
		},
		icon: path.join(__dirname, 'assets', 'icon.ico')
	});

	win.loadFile('./www/index.html');

	win.on('closed', () => {
		win = null;
	});
}

app.on('ready', createWindow);

app.on('window-all-closed', () => {
	if (process.platform !== 'darwin') {
		app.quit();
	}
});

app.on('activate', () => {
	if (win === null) {
		createWindow();
	}
});