import React, { Component } from 'react';
import { GameList } from './GameList';
import { Redirect } from "react-router-dom";


export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);

        this.state = { gameInstances: [], loadingGameInstances: true, loadingExecutions: true };

        fetch('api/instances')
            .then(response => response.json())
            .then(data => {
                this.setState((oldState) => {
                    var newGameInstances = data.reduce((newGameInstances, gameInstance) => {
                        var existingIdx = newGameInstances.findIndex((elem) => elem.key === gameInstance.key);

                        if (existingIdx !== -1) {
                            newGameInstances[existingIdx] = {
                                key: gameInstance.key,
                                save: gameInstance,
                                execution: newGameInstances[existingIdx].execution
                            }
                        } else {
                            newGameInstances.push({
                                key: gameInstance.key,
                                save: gameInstance,
                                execution: null
                            });
                        }
                        return newGameInstances;
                    }, oldState.gameInstances.concat([]))

                    return { gameInstances: newGameInstances, loadingGameInstances: false }
                });
            });

        fetch('api/executions')
            .then(response => response.json())
            .then(data => {
                this.setState((oldState) => {
                    // Merge exectuions in
                    var newGameInstances = data.reduce((newGameInstances, gameExecution) => {
                        var existingIdx = newGameInstances.findIndex((elem) => elem.key === gameExecution.instanceKey);

                        if (existingIdx !== -1) {
                            newGameInstances[existingIdx] = {
                                key: gameExecution.imageKey,
                                save: newGameInstances[existingIdx].save,
                                execution: gameExecution
                            }
                        } else {
                            newGameInstances.push({
                                key: gameExecution.instanceKey,
                                save: null,
                                execution: gameExecution
                            });
                        }

                        return newGameInstances;
                    }, newGameInstances = oldState.gameInstances.concat([]))

                    return { gameInstances: newGameInstances, loadingExecutions: false }
                });
            });

    }

    onStartInstance(game) {
        this.setState({
            startingInstance: game.key
        });
    }

    onStopInstance(game) {
        fetch("/api/executions/" + game.key, {
            method: "DELETE",
            headers: {
                'Content-Type': 'application/json',
            }
        })
            .then(() => {
                // Reload executions
            })

    }

    onRestartInstance(game) {

    }

    render() {
        if (this.state.startingInstance) {
            return (<Redirect to={`start/?save=${this.state.startingInstance}`} />)
        }

        return (
            <GameList
                loading={this.state.loadingExecutions || this.state.loadingGameInstances}
                games={this.state.gameInstances}
                onStartInstance={(game) => this.onStartInstance(game)}
                onStopInstance={(game) => this.onStopInstance(game)}
                onRestartInstance={(game) => this.onRestartInstance(game)}
            />
        );
    }
}
