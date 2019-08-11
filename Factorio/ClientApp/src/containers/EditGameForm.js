import React from 'react';
import Button from 'react-bootstrap/Button';
import Col from 'react-bootstrap/Col';
import Form from 'react-bootstrap/Form';
import Row from 'react-bootstrap/Row';
import { connect } from 'react-redux';
import { abortEditingGame, editingGameSaveChanges, onEditingGameChange } from '../actions/editingGameActions';


export function EditServer({ dispatch, editingGame, originalGame }) {
    return (<div>
        <Form>
            <h1>Editing: {originalGame.name}</h1>

            <Form.Group >
                <Form.Label>Server Name</Form.Label>
                <Form.Control
                    type="text"
                    value={editingGame.name}
                    onChange={(event) => dispatch(onEditingGameChange("name", event.target.value))}
                />
            </Form.Group>
            <Form.Group>
                <Form.Label>Description</Form.Label>
                <Form.Control
                    type="text"
                    value={editingGame.description}
                    onChange={(event) => dispatch(onEditingGameChange("description", event.target.value))}
                />
            </Form.Group>

            < Form.Group >
                <Form.Label>Target Version</Form.Label>
                <Form.Control
                    placeholder="major"
                    value={editingGame.targetVersion.major}
                    onChange={(event) => dispatch(onEditingGameChange("targetVersion.major", event.target.value))}
                />
                <Form.Control
                    placeholder="minor"
                    value={editingGame.targetVersion.minor}
                    onChange={(event) => dispatch(onEditingGameChange("targetVersion.minor", event.target.value))}
                    disabled={editingGame.targetVersion.major == null}
                />
                <Form.Control
                    placeholder="patch"
                    value={editingGame.targetVersion.patch}
                    onChange={(event) => dispatch(onEditingGameChange("targetVersion.patch", event.target.value))}
                    disabled={editingGame.targetVersion.minor == null}
                />
            </Form.Group >
            <Button onClick={(event) => dispatch(editingGameSaveChanges(editingGame))} >Save</Button>
            <Button onClick={() => dispatch(abortEditingGame())}>Cancel</Button>
        </Form>
    </div>);
}

function mapStateToProps(state) {
    const { editingGameReducer } = state; // Grab the reducers

    // Grab the state from them
    const { editingGame, originalGame } = editingGameReducer;

    return {
        editingGame,
        originalGame
    }
}

export default connect(mapStateToProps)(EditServer);