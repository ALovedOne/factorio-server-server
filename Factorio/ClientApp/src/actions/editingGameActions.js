import { Game } from '../models';
import { EDITING_DONE_SAVING, EDITING_GAME_ABORT, EDITING_GAME_BEGIN, EDITING_GAME_VALUE_CHANGE } from "./actionType";

// Bring up the editing form
export function beginEditingGame(game) {
    return {
        type: EDITING_GAME_BEGIN,
        game: game || new Game()
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

// Close the form
export function doneSavingGame(updatedGame) {
    return {
        type: EDITING_DONE_SAVING,
        game: updatedGame
    };
}