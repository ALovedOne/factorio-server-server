import PropTypes from 'prop-types';
import React from 'react';
import Button from 'react-bootstrap/Button';
import CardColumns from 'react-bootstrap/CardColumns';
import { connect } from 'react-redux';
import GameCard from './GameCard';


function GameList({ gameList }) {
    return (
        <CardColumns>
            {gameList.map(game => <GameCard game={game} key={game.key} />)}
        </CardColumns>

    );
}
export default connect()(GameList);


GameList.propTypes = {
    gameList: PropTypes.arrayOf(PropTypes.object).isRequired,
}