import { connect } from 'react-redux';
import { beginEditingGame } from '../actions/editingGameActions';
import GameList from '../components/GameList';

function mapStateToProps({ gameListReducer }) {
    var { gameList } = gameListReducer;

    return { gameList };
};

const mapDispatchToProps = dispatch => ({
    beginEditingGame: () => dispatch(beginEditingGame(null)),
})

export default connect(
    mapStateToProps,
    mapDispatchToProps)(GameList);