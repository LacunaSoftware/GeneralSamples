const { ConnectionBuilder } = require('electron-cgi');
const { app } = require('electron');

class CGI {

	constructor(mode = 'debug', persistent = true) {
		this.mode = mode;
		this.persistent = persistent;
		this.disconnectListeners = [];

		this.buildConnection();

		this._conn.onDisconnect = () => {
			this.disconnectListeners.forEach(handler => handler());
			if (this.persistent) this.buildConnection();
		}
	}

	buildConnection() {
		var path = require('path').normalize(app.getAppPath());

		if (this.mode === 'release') {
			this._conn = new ConnectionBuilder()
				.connectTo(path + '/DotnetCGI/bin/release/netcoreapp2.2/win10-x64/DotnetCGI.exe')
				.build();
		} else {
			this._conn = new ConnectionBuilder()
				.connectTo('dotnet', 'run', '--project', path + '/DotnetCGI')
				.build();
		}
	}

	endConnection() {
		this._conn.close();
	}

	sendRequest(name, args, callback) {
		try {
			this._conn.send(name, args, callback);
		} catch (err) {
			throw err;
		}
	}

	listenDisconnect(onDisconnect) {
		this.disconnectListeners.push(onDisconnect);
	}

	ping(callback) {
		this._conn.send('ping', null, (response, err) => {
			if (err) throw err;

			if (response !== 'pong') throw response;

			callback();
		});
	}
}

module.exports.CGI = CGI;