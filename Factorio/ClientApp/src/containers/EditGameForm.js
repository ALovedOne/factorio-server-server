import React from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { connect } from 'react-redux';
import { abortEditingGame, editingGameSaveChanges, onEditingGameChange } from '../actions/editingGameActions';


export function EditServer({ editingGame, originalGame, onFieldChange, onRequestSave, onAbort }) {
    return (<div>
        <Form>
            <h1>Editing: {originalGame.name}</h1>

            <Form.Group >
                <Form.Label>Server Name</Form.Label>
                <Form.Control
                    type="text"
                    value={editingGame.name}
                    onChange={(event) => onFieldChange("name", event.target.value)}
                />
            </Form.Group>
            <Form.Group>
                <Form.Label>Description</Form.Label>
                <Form.Control
                    type="text"
                    value={editingGame.description}
                    onChange={(event) => onFieldChange("description", event.target.value)}
                />
            </Form.Group>

            < Form.Group >
                <Form.Label>Target Version</Form.Label>
                <Form.Control
                    placeholder="major"
                    value={editingGame.targetVersion.major}
                    onChange={(event) => onFieldChange("targetVersion.major", event.target.value)}
                />
                <Form.Control
                    placeholder="minor"
                    value={editingGame.targetVersion.minor}
                    onChange={(event) => onFieldChange("targetVersion.minor", event.target.value)}
                    disabled={editingGame.targetVersion.major == null}
                />
                <Form.Control
                    placeholder="patch"
                    value={editingGame.targetVersion.patch}
                    onChange={(event) => onFieldChange("targetVersion.patch", event.target.value)}
                    disabled={editingGame.targetVersion.minor == null}
                />
            </Form.Group >
            <Button onClick={() => onRequestSave(editingGame)} >Save</Button>
            <Button onClick={() => onAbort()}>Cancel</Button>
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

function mapDispatchToProps(dispatch, ownProps) {
    return {
        onFieldChange: (field, newValue) => {
            dispatch(onEditingGameChange(field, newValue))
        },
        onRequestSave: (game) => dispatch(editingGameSaveChanges(game)),
        onAbort: () => dispatch(abortEditingGame())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(EditServer);