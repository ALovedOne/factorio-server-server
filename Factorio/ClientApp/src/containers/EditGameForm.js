import { Formik } from 'formik';
import PropTypes from 'prop-types';
import React from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { connect } from 'react-redux';
import { abortEditingGame, doneSavingGame } from '../actions/editingGameActions';
import * as GameServer from '../services/gameInstances';


const EditServerDialog = ({ server, onUpdateServer, onClose }) => {
    return (
        <div>
            <h1>Editing: {server.name}</h1>
            <Formik
                initialValues={server}
                onSubmit={(values, actions) => {
                    GameServer.requestSaveGame(values)
                        .then(updatedGame => {
                            actions.setSubmitting(false);
                            // Dispatch
                            onUpdateServer(updatedGame);
                            onClose();
                        },
                        error => {
                                // TODO
                                actions.setSubmitting(false);
                                actions.setErrors(error);
                                actions.setStatus({ msg: 'Set some arbitrary status or data' });
                            })
                }}
                render={({
                    values,
                    errors,
                    status,
                    touched,
                    handleBlur,
                    handleChange,
                    handleSubmit,
                    isSubmitting,
                }) => (
                        <Form onSubmit={handleSubmit} >
                            <Form.Group >
                                <Form.Label>Server Name</Form.Label>
                                <Form.Control
                                    type="text"
                                    name="name"
                                    onChange={handleChange}
                                    onBlur={handleBlur}
                                    value={values.name}
                                />
                            </Form.Group>
                            {errors.name && touched.name && <div>{errors.name}</div>}

                            <Form.Group>
                                <Form.Label>Description</Form.Label>
                                <Form.Control
                                    type="text"
                                    name="description"
                                    onChange={handleChange}
                                    onBlur={handleBlur}
                                    value={values.description}
                                />
                            </Form.Group>
                            {errors.description && touched.description && <div>{errors.description}</div>}

                            < Form.Group >
                                <Form.Label>Target Version</Form.Label>
                                <Form.Control
                                    type="number"
                                    name="targetVersion.major"
                                    onChange={handleChange}
                                    onBlur={handleBlur}
                                    value={values.targetVersion.major}

                                    placeholder="major"
                                />
                                <Form.Control
                                    type="number"
                                    name="targetVersion.minor"
                                    onChange={handleChange}
                                    onBlur={handleBlur}
                                    value={values.targetVersion.minor}

                                    placeholder="minor"
                                    disabled={values.targetVersion.major == null}
                                />
                                <Form.Control
                                    type="number"
                                    name="targetVersion.patch"
                                    onChange={handleChange}
                                    onBlur={handleBlur}
                                    value={values.targetVersion.patch}


                                    placeholder="patch"
                                    disabled={values.targetVersion.minor == null}
                                />
                            </Form.Group >
                            {status && status.msg && <div>{status.msg}</div>}
                            <Button type="submit" disabled={isSubmitting}>Save</Button>
                        </Form>
                    )}
            />  
        </div>
    );
}

function mapStateToProps(state) {
    const { editingGameReducer } = state; // Grab the reducers

    // Grab the state from them
    const { editingGame } = editingGameReducer;

    return {
        editingGame
    }
}


function mapDispatchToProps(dispatch, ownProps) {
    return {
        onUpdateServer: (game) => dispatch(doneSavingGame(game)),
        onClose: () => dispatch(abortEditingGame())
    }
}

EditServerDialog.propTypes = {
    server: PropTypes.object.isRequired,
    onUpdateServer: PropTypes.func.isRequired,
    onClose: PropTypes.func.isRequired
}

export default connect(mapStateToProps, mapDispatchToProps)(EditServerDialog);