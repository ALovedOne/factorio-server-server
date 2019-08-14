import { connect } from 'react-redux';
import { Home } from '../components/Home';
import { beginEditingGame } from '../actions/editingGameActions';

function mapStateToProps(state) {
    const { gameListReducer, launchingGameReducer, editingGameReducer } = state;

    const { gameList } = gameListReducer;
    const { editingGame, originalGame } = editingGameReducer;
    const { launchingGame } = launchingGameReducer;

    return {
        gameList,
        launchingGame,
        editingGame,
        originalGame,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        onAddNew: () => dispatch(beginEditingGame())
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(Home);