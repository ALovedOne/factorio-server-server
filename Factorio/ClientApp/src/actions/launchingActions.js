import { LAUNCHING_GAME_ABORT, LAUNCHING_GAME_BEGIN, STARTING_GAME, UPDATE_GAME_INFO, UPDATE_LAUNCHING_PORT } from "./actionType";

export function beginStartingGame(gameKey) {
    return {
        type: STARTING_GAME,
        gameKey: gameKey
    };
}

export function startGame(game, port) {
    return dispatch => {
        dispatch(beginStartingGame({ gameKey: game.key }));
        fetch("api/instances/" + game.key + "/start?port=" + port, {
            method: "POST"
        })
            .then((resp) => resp.json())
            .then((data) => {
                dispatch(endStartingStoppingGame(game.key, data));
            });
    }
}

export function stopGame(game) {

    return dispatch => {
        dispatch(beginStartingGame({ gameKey: game.key }));
        fetch("api/instances/" + game.key + "/stop", {
            method: "POST"
        })
            .then((resp) => resp.json())
            .then((data) => {
                dispatch(endStartingStoppingGame(game.key, data));
            });
    }
}


export function restartGame(game) {
    return dispatch => {
        // TODO - initial update
        dispatch(beginStartingGame({ gameKey: game.key }));
        fetch("api/instances/" + game.key + "/restart", {
            method: "POST"
        })
            .then((resp) => resp.json())
            .then((data) => {
                dispatch(endStartingStoppingGame(game.key, data));
            });
    }
}


function endStartingStoppingGame(gameKey, newGameInfo) {
    console.assert(newGameInfo);
    return {
        type: UPDATE_GAME_INFO,
        gameKey: gameKey,
        gameInfo: newGameInfo
    };
}

//#region Launching Game
export function beginLaunchingGame(game) {
    return {
        type: LAUNCHING_GAME_BEGIN,
        game: game
    };
}

export function abortLaunchingGame(game) {
    return {
        type: LAUNCHING_GAME_ABORT,
        game: game
    };
}

export function updateLaunchingPort(game, port) {
    return {
        type: UPDATE_LAUNCHING_PORT,
        game: game,
        port: port
    };
}
//#endregion