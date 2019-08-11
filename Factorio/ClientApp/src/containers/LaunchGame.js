import React from 'react';
import Button from 'react-bootstrap/Button';
import MdPlay from 'react-icons/lib/md/play-arrow';
import { connect } from 'react-redux';
import { beginLaunchingGame } from '../actions/launchingActions';

function renderLaunchGame({ game, onClick }) {
    return (<Button variant="link" onClick={() => onClick(game)}><MdPlay size={24} /></Button>);
}

function mapDispatchToProps(dispatch, ownProps) {
    return {
        onClick: (game) => dispatch(beginLaunchingGame(game))
    };
}

export default connect(null, mapDispatchToProps)(renderLaunchGame);