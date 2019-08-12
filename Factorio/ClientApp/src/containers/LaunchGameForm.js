import React from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { connect } from 'react-redux';
import * as launchingGameActions from '../actions/launchingActions';

function LaunchGameFormComponent({ game, info, onPortChange, doLaunch, abortLaunch }) {
    return <div>
        <Form>
            <h1> Launching: {game.name}</h1>
            <Form.Group>
                <Form.Label>Port</Form.Label>
                <Form.Control
                    type="number"
                    value={info.port}
                    onChange={(event) => onPortChange(event.target.value)} />
            </Form.Group>

            <Button onClick={() => doLaunch(game, info)} >Launch</Button>
            <Button onClick={() => abortLaunch(game)}>Cancel</Button>
        </Form>
    </div>;
}

function mapStateToProps({ launchingGameReducer }) {
    return {
        game: launchingGameReducer.launchingGame,
        info: launchingGameReducer.info,
    };
}

function mapDispatchToProps(dispatch, ownProps) {
    return {
        doLaunch: (game, info) => dispatch(launchingGameActions.initiateStartingGame(game, info)),
        abortLaunch: (game) => dispatch(launchingGameActions.abortLaunchingGame(ownProps.game)),
        onPortChange: (newValue) => dispatch(launchingGameActions.updateLaunchingGameInfo(ownProps.game, newValue))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(LaunchGameFormComponent);