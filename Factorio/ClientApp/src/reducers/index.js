import { combineReducers } from 'redux';
import editingGameReducer from './editGameReducer';
import gameListReducer from './gameListReducer';
import launchingGameReducer from './launchGameReducer';

export default combineReducers({
    gameListReducer,  
    editingGameReducer,
    launchingGameReducer,
})