import { AxiosResponse } from "axios";

export default class ClientsBase {

  protected transformOptions(options: any): Promise<any> {
    const token = localStorage.getItem('accessToken');

    if (token) {
      options.headers = options.headers || {};
      options.headers["Authorization"] = `Bearer ${token}`;
    }

    return Promise.resolve(options);
  }

  transformResult (url: string, response: AxiosResponse<any>, processor: (response: AxiosResponse<any>) => Promise<any>): Promise<any> {
    // hack, in order to simulate that response is a string :/ HACK! VERY BAD!
    if (response.data) {
      response.data = JSON.stringify(response.data)
    }
    return processor(response)
  }
}
