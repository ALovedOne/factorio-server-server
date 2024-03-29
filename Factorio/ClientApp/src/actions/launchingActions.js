﻿import * as gameInstanceService from '../services/gameInstances';
import { LAUNCHING_GAME_ABORT, LAUNCHING_GAME_BEGIN, LAUNCHING_GANE_DONE } from "./actionType";

/*
 * 1) beginStartingGame -> bring up form
 * 2) updateLaunchingInfo -> update with launch info
 * x) abortLaunching -> bring down form
 * 3) initiateStartingGame -> 
 *      - updates state to show progress
 *      - Make call to start
 * 4) restartGame,
 *      - bring up progressbar
 *      - make call to restart
 * 5) stopGame,
 *      - bring up progressbar
 *      - make call to stop...
 * *) in callbacks,
 *      - disable progress bar
 *      - update game list state
 */

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
    }
}

export function restartGame(game) {
    return dispatch => {
        gameInstanceService.requestRestartGame(game)
            .then((updatedGameModel) => dispatch(launchingGameDone(updatedGameModel)));
    }
}

export function stopGame(game) {
    return dispatch => {
        gameInstanceService.requestStopGame(game)
            .then((updatedGameModel) => dispatch(launchingGameDone(updatedGameModel)));
    }
}

export function launchingGameDone(game) {
    return {
        type: LAUNCHING_GANE_DONE,
        game: game
    }
}

