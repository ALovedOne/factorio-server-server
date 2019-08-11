import { EDITING_BEGIN_SAVING, EDITING_DONE_SAVING, EDITING_GAME_ABORT, EDITING_GAME_BEGIN, EDITING_GAME_VALUE_CHANGE, EDITING_GAME_MOD_VALUE_CHANGE} from "./actionType";
import { onLoadingSuccess } from './index';


// Bring up the editing form
export function beginEditingGame(game) {
    return {
        type: EDITING_GAME_BEGIN,
        game: game
    };
}

// Close the editing form without saving
export function abortEditingGame() {
    return {
        type: EDITING_GAME_ABORT,
    }
}

// Field changed on the form
export function onEditingGameChange(field, value) {
    return {
        type: EDITING_GAME_VALUE_CHANGE,
        field: field,
        value: value
    };
}

export function editingGameSaveChanges(game) {
    return dispatch => {

        const url = game.key ? `api/instances/${game.key}` : `api/instances`;
        const method = game.key ? 'PUT' : 'POST';

        dispatch(beginSavingGame());
        fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(game)
        })
            .then(response => response.json())
            .then((data) => {
                dispatch(onLoadingSuccess(data));
                dispatch(doneSavingGame());
            });
    };
}

// Bring let the user know we are saving
function beginSavingGame() {
    return {
        type: EDITING_BEGIN_SAVING
    };
}

// Close the form
function doneSavingGame() {
    return {
        type: EDITING_DONE_SAVING
    };
}
//#endregion

