//import { LOAD_INSTANCES_BEGIN, LOAD_INSTANCES_END } from "../actions/actionType";

const LOAD_INSTANCES_BEGIN = "LOAD_INSTANCES_BEGIN";
const LOAD_INSTANCES_END = "LOAD_INSTANCES_END";
const UPDATE_GAME_INFO = "UPDATE_GAME_INFO";

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

    if (action.type === UPDATE_GAME_INFO) {
        var { gameKey, gameInfo } = action;

        newGameList = state.gameList.concat([]).map((g) => {
            if (g.key !== gameKey) return g;
            else return gameInfo;
        });

        return Object.assign({}, state, {
            games: newGameList
        });
    }
    // debugger;
    return state;
}