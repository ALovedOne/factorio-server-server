import React from 'react';
import Button from 'react-bootstrap/Button';
import MdEdit from 'react-icons/lib/md/edit';
import { connect } from 'react-redux';
import { beginEditingGame } from '../actions/editingGameActions';


function renderEditServer({ game, onClick }) {
    return (<Button variant="link" onClick={() => onClick(game)}><MdEdit size={24} /></Button>);
}

function mapDispatchToProps(dispatch, ownProps) {
    return {
        onClick: (game) => dispatch(beginEditingGame(game))
    }
}

export default connect(null, mapDispatchToProps)(renderEditServer);