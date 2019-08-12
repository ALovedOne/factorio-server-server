

export function requestStartGame(game, port) {
    return fetch("/api/instances/" + game.key + "/start?port=" + port, {
        method: "POST"
    })
        .then((resp) => resp.json())
    // TODO - convert to model
}

export function requestRestartGame(game) {
    return fetch("api/instances/" + game.key + "/restart", {
        method: "POST"
    })
        .then((resp) => resp.json())
    // TODO - convert to model

}

export function requestStopGame(game) {
    return fetch("api/instances/" + game.key + "/stop", {
        method: "POST"
    })
        .then((resp) => resp.json())
    // TODO - convert to model
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
        .then(response => response.json());
    // TODO - convert to model
}