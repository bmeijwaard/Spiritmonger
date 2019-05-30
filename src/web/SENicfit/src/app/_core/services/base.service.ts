import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ApiResponse } from '../models/responses/api.response';

@Injectable()
export abstract class BaseService {
    protected _isLoading: BehaviorSubject<boolean>;

    constructor(protected readonly http: HttpClient, @Inject('BASE_URL') protected readonly baseUrl: string) {
        this._isLoading = <BehaviorSubject<boolean>>new BehaviorSubject(false);
    }

    get isLoading() {
        return this._isLoading.asObservable();
    }

    protected async handleError(error: HttpErrorResponse): Promise<any> {
        return await Promise.reject(error);
    }

    protected _handleError(error: any) {
        console.log(`error: ${error}`);
        this._isLoading.next(false);
    }

    protected get = async <T = null>(path: string): Promise<ApiResponse<T>> => {
        this._isLoading.next(true);
        return await this.http
            .get(`${this.baseUrl}${path}`)
            .toPromise()
            .then(response => {
                if ((<ApiResponse<T>>response).success) {
                    this._isLoading.next(false);
                    return <ApiResponse<T>>response;
                } else {
                    this._handleError((<ApiResponse<T>>response).error);
                    return <ApiResponse<T>>response;
                }
            })
            .catch(error => {
                this._handleError(JSON.stringify(error));
                return <ApiResponse<T>>{
                    error: JSON.stringify(error.error),
                    message: (<any>error).message || '',
                    success: (<any>error).ok
                };
            });
    };

    protected post = async <T = null>(path: string, body: any): Promise<ApiResponse<T>> => {
        console.log('body', body);
        this._isLoading.next(true);
        return await this.http
            .post(`${this.baseUrl}${path}`, body)
            .toPromise()
            .then(response => {
                if ((<ApiResponse<T>>response).success) {
                    this._isLoading.next(false);
                    return <ApiResponse<T>>response;
                } else {
                    this._handleError((<ApiResponse<T>>response).error);
                    return <ApiResponse<T>>response;
                }
            })
            .catch(error => {
                this._handleError(JSON.stringify(error));
                return <ApiResponse<T>>{
                    error: JSON.stringify(error.error),
                    message: (<any>error).message || '',
                    success: (<any>error).ok
                };
            });
    };

    protected put = async <T = null>(path: string, body: any): Promise<ApiResponse<T>> => {
        console.log('body', body);
        this._isLoading.next(true);
        return await this.http
            .put(`${this.baseUrl}${path}`, body)
            .toPromise()
            .then(response => {
                if ((<ApiResponse<T>>response).success) {
                    this._isLoading.next(false);
                    return <ApiResponse<T>>response;
                } else {
                    this._handleError((<ApiResponse<T>>response).error);
                    return <ApiResponse<T>>response;
                }
            })
            .catch(error => {
                this._handleError(JSON.stringify(error));
                return <ApiResponse<T>>{
                    error: JSON.stringify(error.error),
                    message: (<any>error).message || '',
                    success: (<any>error).ok
                };
            });
    };
}
