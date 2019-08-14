import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import * as actions from '../actions/editingGameActions';
import * as types from '../actions/actionType';
import fetchMock from 'fetch-mock';
import expect from 'expect';
import { Game, Version } from '../models';

const middleware = [thunk];
const mockStore = configureMockStore(middleware);

describe('editingGame actions', () => {
    afterEach(() => {
        fetchMock.restore();
    })

    it('does stuff', () => {
        fetchMock.put('api/instances/key', {
            body: {
                key: "key",
                name: "name",
                description: "description",
                mods: [],
                targetVersion: {},

            },
            headers: { 'content-type': 'application/json' }
        });

        const game = new Game("key", "name", "description", new Version(0, 17, 59));

        const expectedActions = [
            {
                type: types.EDITING_BEGIN_SAVING
            },
            {
                type: types.EDITING_DONE_SAVING,
                game: {}
            }];

        const expectedHTTPCalls = [
            ['/api/instances/test', game, { method: "PUT" }]];
        const store = mockStore();

        return store.dispatch(actions.editingGameSaveChanges(game)).then(() => {
            expect(fetchMock.calls()).toEqual(expectedHTTPCalls);
            expect(store.getActions()).toEqual(expectedActions);

        })
    })
})