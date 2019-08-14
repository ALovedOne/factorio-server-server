import { Game } from '../models';

export function requestStartGame(game, port) {
    return fetch("/api/instances/" + game.key + "/start?port=" + port, {
        method: "POST"
    })
        .then((resp) => resp.json())
        .then(Game.fromJson);
}

export function requestRestartGame(game) {
    return fetch("api/instances/" + game.key + "/restart", {
        method: "POST"
    })
        .then((resp) => resp.json())
        .then(Game.fromJson);
}

export function requestStopGame(game) {
    return fetch("api/instances/" + game.key + "/stop", {
        method: "POST"
    })
        .then((resp) => resp.json())
        .then(Game.fromJson);
}

export function requestSaveGame(game) {
    const url = game.key ? `api/instances/${game.key}` : `api/instances`;
    const method = game.key ? 'PUT' : 'POST';


    return fetch(url, {
        method: method,
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(game)
    })
        .then(response => response.json())
        .then(Game.fromJson);
}

export function requestLoadAllGames() {
    return fetch('api/instances')
        .then(resp => resp.json())
        .then(resp => resp.map(Game.fromJson));
}