import * as actionTypes from '../actions/actionType';

const LAUNCHING_GAME_BEGIN = "LAUNCHING_GAME_BEGIN";
const LAUNCHING_GAME_ABORT = "LAUNCHING_GAME_ABORT";
const UPDATE_LAUNCHING_PORT = "UPDATE_LAUNCHING_PORT";

const initialState = {
    launchingGame: null,
    port: null
}

export default function launchingGameReducer(state, action) {
    if (typeof state === 'undefined') {
        return initialState;
    }

    if (action.type === LAUNCHING_GAME_BEGIN) {
        return Object.assign({}, state, { launchingGame: action.game });
    }

    if (action.type === actionTypes.LAUNCHING_GAME_ABORT) {
        return initialState;
    }

    if (action.type === actionTypes.UPDATE_LAUNCHING_PORT) {
        return Object.assign({}, state, { port: action.port });
    }



    // debugger;
    return state;
}