import { LOAD_INSTANCES_BEGIN, LOAD_INSTANCES_END, UPDATE_GAME_INFO, LAUNCHING_GANE_DONE, EDITING_DONE_SAVING } from "../actions/actionType";

const initialState = {
    gameList: [],
    loading: false,
}

export default function gameListReducer(state, action) {
    var newGameList;

    if (typeof state === 'undefined') {
        return initialState;
    }

    if (action.type === LOAD_INSTANCES_BEGIN) {

        return Object.assign({}, state, {
            gameList: [],
            loading: true,
        });
    }

    if (action.type === LOAD_INSTANCES_END) {
        var { response } = action.payload;

        if (Array.isArray(response)) {
            return Object.assign({}, state, {
                gameList: response,
                loading: false,
            });
        } else {

            if (state.gameList.find(g => g.key === response.key)) {
                newGameList = state.gameList.map((g) => g.key === response.key ? response : g)
            } else {
                newGameList = state.gameList.concat([response]);
            }
            return Object.assign({}, state, {
                gameList: newGameList
            });
        }
    }

    if (action.type === LAUNCHING_GANE_DONE || action.type === EDITING_DONE_SAVING) {
        var { game } = action;

        return Object.assign({}, state, {
            gameList: _replaceGame(state.gameList, game)
        })
    }
    // debugger;
    return state;
}

function _replaceGame(gameList, game) {
    if (gameList.find(x => x.key === game.key)) {
        return gameList.map((g) => {
            if (g.key !== game.key) return g;
            else return game;
        });
    } else {
        return gameList.concat([game]);
    }
}