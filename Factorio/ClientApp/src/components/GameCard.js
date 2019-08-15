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

function GameCard({ game }) {
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
        var connectionInfo = <Card.Text>{`Host: ${execution.hostname}:${execution.port}`}</Card.Text>
    }

    if (game.lastSave) {
        var saveVersion = <span>(Last Save: <Version version={game.lastSave.version} />)</span>;
    }

    if (game.mods.length > 0) {
        var modsList = `Mods: ${game.mods.length}`
    }

    // Add save.mods
    return (
        <Card >
            <Card.Img variant="top" src={PlaceHolderImage} />
            <Card.Body>
                <Card.Title>{game.name}</Card.Title>
                <Card.Subtitle><Version version={game.targetVersion} /> {runningVersion ? runningVersion : saveVersion ? saveVersion : ""}</Card.Subtitle>
                {connectionInfo}
                <Card.Text>{game.description}</Card.Text>
                {modsList}
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