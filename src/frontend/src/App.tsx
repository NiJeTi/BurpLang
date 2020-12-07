import React, {useState} from 'react';

import './App.css';

import Input from './components/Input';
import Output from './components/Output';

import initialResponse, {ApiResponse} from './models/apiResponse';

function App() {
  const [ responseState, setResponseState ] = useState<ApiResponse>(initialResponse);

  const onPost = (data: string) =>
    postData(data)
      .then(setResponseState);

  return (
    <div id="terminal">
      <Input
        onValueChange={onPost}
        error={responseState.error}
      />
      <Output
        response={responseState}
      />
    </div>
  );
}

function postData(data: string): Promise<ApiResponse> {
  const request: RequestInit = {
    method: 'POST',
    headers: {
      'Content-Type': 'text/plain'
    },
    body: data
  };

  return fetch('http://localhost:5000/parse', request)
    .then(response => response.json());
}

export default App;
