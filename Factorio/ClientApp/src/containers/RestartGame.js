import React from 'react';
import Button from 'react-bootstrap/Button';
import MdSync from 'react-icons/lib/md/sync';
import { connect } from 'react-redux';
import * as launchingActions from '../actions/launchingActions';


function render({ game, onClick }) {
    return (<Button variant="link" onClick={() => onClick(game)}><MdSync size={24} /></Button>);
}

function mapDispatchToProps(dispatch, ownProps) {
    return { onClick: (game) =>  dispatch(launchingActions.restartGame(game))}
}


export default connect(null, mapDispatchToProps)(render);