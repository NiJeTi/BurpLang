interface ApiResponse {
  entity: Record<string, any> | null,
  error: ParsingError | null
}

interface ParsingError {
  message: string,

  startIndex: number,
  endIndex: number
}

const initialResponse: ApiResponse = {
  entity: null,
  error: null,
};

export type {ApiResponse, ParsingError};
export default initialResponse;
