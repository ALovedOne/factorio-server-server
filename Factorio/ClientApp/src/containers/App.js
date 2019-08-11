import { connect } from 'react-redux';
import { loadAllGames } from '../actions';
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

const mapDispatchToProps = dispatch => ({
});


export default connect(mapStateToProps, mapDispatchToProps)(Home);