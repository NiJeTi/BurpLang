import React from 'react';
import ReactJson from 'react-json-view';
import initialResponse, {ApiResponse} from '../models/apiResponse';
import './Output.css';

interface OutputProps {
  response: ApiResponse;
}

function Output(props: OutputProps) {
  if (props.response.entity === initialResponse.entity &&
    props.response.error === initialResponse.error) {
    return <div id="output"/>;
  }

  if (props.response.error != null) {
    return (
      <div id="output">
        <p className="errorMessage">{props.response.error?.message}</p>
      </div>
    );
  }

  return (
    <div id="output">
      <ReactJson src={props.response.entity as object}/>
    </div>
  );
}

export default Output;