import { Formik } from 'formik';
import PropTypes from 'prop-types';
import React from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { connect } from 'react-redux';
import * as launchingGameActions from '../actions/launchingActions';
import * as GameServer from '../services/gameInstances';

const LaunchGameDialog = ({ game, onLaunch, onClose }) => {
    return (
        <div>
            <h1>Launching: {game.name}</h1>
            <Formik
                initialValues={{}}
                onSubmit={(values, actions) => {
                    GameServer.requestStartGame(game, values.port)
                        .then(updatedGame => {
                            actions.setSubmitting(false);
                            onLaunch(updatedGame);
                            onClose();
                        },
                            error => {
                                // TODO
                                actions.setSubmitting(false);
                                actions.setErrors(error);
                                actions.setStatus({ msg: 'Set some arbitrary status or data' });
                            });
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
                        <Form onSubmit={handleSubmit}>
                            <Form.Group>
                                <Form.Label>Port</Form.Label>
                                <Form.Control
                                    type="number"
                                    name="port"
                                    onChange={handleChange}
                                    onBlur={handleBlur}
                                    value={values.name}
                                />
                            </Form.Group>
                            {errors.port && touched.port && <div>{errors.port}</div>}

                            <Button type="submit" disabled={isSubmitting}>Launch</Button>
                        </Form>
                    )
                }
            />
        </div>)
}
function mapStateToProps({ launchingGameReducer }) {
    return {
        game: launchingGameReducer.launchingGame,
        info: launchingGameReducer.info,
    };
}

function mapDispatchToProps(dispatch, ownProps) {
    return {
        doLaunch: (game, info) => dispatch(launchingGameActions.launchingGameDone(game)),
        onClose: (game) => dispatch(launchingGameActions.abortLaunchingGame(ownProps.game))
   }
}

LaunchGameDialog.propTypes = {
    game: PropTypes.object.isRequired,
    onLaunch: PropTypes.func.isRequired,
    onClose: PropTypes.func.isRequired
}

export default connect(mapStateToProps, mapDispatchToProps)(LaunchGameDialog);