import PropTypes from 'prop-types';
import React from 'react';
import Card from 'react-bootstrap/Card';
import Col from 'react-bootstrap/Col';
import Row from 'react-bootstrap/Row';
import { connect } from 'react-redux';
import PlaceHolderImage from '../assets/spagetti-rocket.jpg';
import EditServer from '../containers/EditServer';
import LaunchGame from '../containers/LaunchGame';
import RestartGame from '../containers/RestartGame';
import StopGame from '../containers/StopGame';
import Version from './Version';

function GameCard({ game, beginEditingGame, beginLaunchingGame, onStopGame, onRestartGame }) {
    var execution = game.currentExecution;


    var footer = (
        <Col>
            <LaunchGame game={game} />
        </Col>);

    if (execution) {
        footer = (
            <Col>
                <StopGame game={game} />
                <RestartGame game={game} />
            </Col>);
        var runningVersion = <span>{` (Running:  ${execution.runningVersion} )`}</span>;
    }

    if (game.lastSave) {
        var saveVersion = <span>(Last Save: <Version version={game.lastSave.version} />)</span>;
    }

    // Add save.mods
    return (
        <Card >
            <Card.Img variant="top" src={PlaceHolderImage} />
            <Card.Body>
                <Card.Title>{game.name}</Card.Title>
                <Card.Subtitle><Version version={game.targetVersion} /> {runningVersion ? runningVersion : saveVersion ? saveVersion : ""}</Card.Subtitle>
                <Card.Text>
                    {game.description}
                </Card.Text>
                <Card.Text>
                    Mod Count: {game.mods.length}
                </Card.Text>
                <Card.Text>
                    Connection Info
                </Card.Text>
            </Card.Body>
            <Card.Footer>
                <Row>
                    {footer}
                    <Col>
                        <EditServer game={game} />
                    </Col>
                </Row>
            </Card.Footer>
        </Card >
    );
}


GameCard.propTypes = {
    game: PropTypes.object.isRequired, // TODO - add shape
}
export default connect()(GameCard);