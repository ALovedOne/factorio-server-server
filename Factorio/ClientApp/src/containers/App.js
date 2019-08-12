import { connect } from 'react-redux';
import { Home } from '../components/Home';

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

export default connect(mapStateToProps)(Home);