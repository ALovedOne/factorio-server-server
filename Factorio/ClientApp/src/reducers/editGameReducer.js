const EDITING_GAME_BEGIN = "EDITING_GAME_BEGIN";
const EDITING_GAME_ABORT = "EDITING_GAME_ABORT";
const EDITING_GAME_VALUE_CHANGE = "EDITING_GAME_VALUE_CHANGE";
const EDITING_BEGIN_SAVING = "EDITING_BEGIN_SAVING";
const EDITING_DONE_SAVING = "EDITING_DONE_SAVING";
const EDITING_GAME_MOD_VALUE_CHANGE = "EDITING_GAME_MOD_VALUE_CHANGE";


const initialState = {
    editingGame: null,
    originalGame: null,
    loading: false,
    isSaving: false,
}

export default function editGameReducer(state, action) {
    if (typeof state === 'undefined') {
        return initialState;
    }

    var { game, type, field, value } = action;
    var { editingGame } = state;

    if (type === EDITING_GAME_BEGIN) {
        if (game === undefined) {
            game = {
                name: "",
                description: "",

            }
        }
        return Object.assign({}, state, { editingGame: game, originalGame: game });
    }

    if (type === EDITING_GAME_VALUE_CHANGE) {
        return Object.assign({}, state, { editingGame: replaceFieldInObject(editingGame, field, value) });
    }

    if (type === EDITING_BEGIN_SAVING) {
        return Object.assign({}, state, { isSaving: true });
    }

    if (type === EDITING_DONE_SAVING || type === EDITING_GAME_ABORT) {
        return Object.assign({}, state, { editingGame: null, originalGame: null, isSaving: false });
    }

    if (type === EDITING_GAME_MOD_VALUE_CHANGE) {
        var mod = editingGame.mods.find(m => m.name === action.modName);
        if (mod !== undefined) {
            var newMod = replaceFieldInObject(mod, field, value);

            return Object.assign({}, state, { editingGame: Object.assign({}, editingGame, { mods: editingGame.mods.map(m => m.name === action.modName ? newMod : m) }) });
        }
        else {
            debugger;
        }
    }

    if (type.startsWith("EDITING")) {
        debugger;
    }
    return state;
}

function replaceFieldInObject(object, fieldKeys, value) {
    return replaceFieldInObjectPriv(object, fieldKeys.split("."), value);
}

function replaceFieldInObjectPriv(object, fieldsAry, value) {
    if (fieldsAry.length === 0) return value;

    var objKey = fieldsAry[0];
    var subKeys = fieldsAry.slice(1);

    return Object.assign({}, object, { [objKey]: replaceFieldInObjectPriv(object[objKey], subKeys, value) });
}