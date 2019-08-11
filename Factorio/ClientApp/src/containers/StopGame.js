import React from 'react';
import Button from 'react-bootstrap/Button';
import MdStop from 'react-icons/lib/md/stop';
import { connect } from 'react-redux';
import * as launchingActions from '../actions/launchingActions';

function render({ game, onClick }) {
    return (<Button variant="link" onClick={() => onClick(game)}><MdStop size={24} /></Button>);
}

function mapDispatchToProps(dispatch, ownProps) {
    return { onClick: (game) => dispatch(launchingActions.stopGame(game)) }
}

export default connect(null, mapDispatchToProps)(render);