import React, {useEffect, useRef, useState} from 'react';
import MonacoEditor from 'react-monaco-editor';
import {ParsingError} from '../models/apiResponse';

import './Input.css';

interface InputProps {
  onValueChange: (newValue: string) => void;
  error: ParsingError | null;
}

function Input(props: InputProps) {
  const monacoRef = useRef<MonacoEditor>(null);
  const [ inputState, setInputState ] = useState<string>('');
  const [ decorationState, setDecorationState ] = useState<any>([]);

  useEffect(() => {
    const newDecoration = props.error
      ? getErrorDecoration(inputState, props.error.startIndex, props.error.endIndex)
      : [];

    if (newDecoration.length === decorationState.length) {
      return;
    }

    setDecorationState(monacoRef.current?.editor?.deltaDecorations(decorationState, newDecoration));
  }, [ decorationState, inputState, props.error ]);

  return (
    <div id="input">
      <MonacoEditor
        width={window.innerWidth / 2}
        height={window.innerHeight / 1.1}
        theme="vs-dark"
        onChange={newValue => {
          newValue = newValue.replaceAll('\r\n', '\n');

          setInputState(newValue);
          props.onValueChange(newValue);
        }}
        ref={monacoRef}
      />
    </div>
  );
}

function getErrorDecoration(text: string, start: number, end: number) {
  let startLineNumber = 1;
  let startColumn = 1;
  let endLineNumber = 1;
  let endColumn = 1;

  if (text[start] === '\n') {
    start++;
    end++;
  }

  let i = 0;
  while (i < start) {
    if (text[i] === '\n') {
      startLineNumber++;
      startColumn = 1;
    } else {
      startColumn++;
    }
    i++;
  }

  i = 0;
  while (i < end) {
    if (text[i] === '\r' || text[i] === '\n' || text[i] === '\r\n') {
      endLineNumber++;
      endColumn = 1;
    } else {
      endColumn++;
    }
    i++;
  }

  return [ {
    range: {
      startLineNumber: startLineNumber,
      startColumn: startColumn,
      endLineNumber: endLineNumber,
      endColumn: endColumn
    }, options: { className: 'errorHighlight' }
  }
  ];
}

export default Input;