import { LOAD_INSTANCES_BEGIN, LOAD_INSTANCES_END } from "./actionType";

const successfulAction = (actionType, payload) => {
    return {
        type: actionType,
        payload: payload,
    }
};

const failureAction = (actionType, error) => {
    return {
        type: actionType,
        error: error,
    }
}

export const beginLoadInstance = ({ instanceID = null }) => {
    return successfulAction(LOAD_INSTANCES_BEGIN, { request: { instanceID: instanceID } });
}


export const onLoadingSuccess = ( data) => {
    return successfulAction(LOAD_INSTANCES_END, {
        response: data
    });
}

export const onLoadingFailure = ({ errorReason = null }) => {
    return failureAction(LOAD_INSTANCES_END, errorReason);
}