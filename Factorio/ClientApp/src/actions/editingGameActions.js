import { EDITING_BEGIN_SAVING, EDITING_DONE_SAVING, EDITING_GAME_ABORT, EDITING_GAME_BEGIN, EDITING_GAME_VALUE_CHANGE } from "./actionType";
import { requestSaveGame } from '../services/gameInstances';

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
        dispatch(beginSavingGame());
        requestSaveGame(game).then((updatedModel) => {
            dispatch(doneSavingGame(updatedModel));

        });
    }
}

// Bring let the user know we are saving
function beginSavingGame() {
    return {
        type: EDITING_BEGIN_SAVING
    };
}

// Close the form
function doneSavingGame(updatedGame) {
    return {
        type: EDITING_DONE_SAVING,
        game: updatedGame
    };
}