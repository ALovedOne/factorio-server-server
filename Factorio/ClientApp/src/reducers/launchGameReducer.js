import { LAUNCHING_GAME_BEGIN, LAUNCHING_GAME_ABORT, LAUNCHING_GAME_UPDATE_INFO, LAUNCHING_GAME_IN_PROGRESS, LAUNCHING_GANE_DONE } from "../actions/actionType";


const initialState = {
    launchingGame: null,
    info: {}
}

export default function launchingGameReducer(oldState = initialState, action) {

    if (action.type === LAUNCHING_GAME_BEGIN) {
        return Object.assign({}, oldState, { launchingGame: action.game });
    }

    if (action.type === LAUNCHING_GAME_ABORT) {
        return initialState;
    }

    if (action.type === LAUNCHING_GAME_UPDATE_INFO) {
        return Object.assign({}, oldState, {
            info: {
                port: action.info.port || oldState.info.port,
            }
        });
    }

    if (action.type === LAUNCHING_GANE_DONE) {
        return initialState;
    }
    return oldState;
}