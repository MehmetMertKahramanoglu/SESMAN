export interface BaseDto

{
    id: string;
    createdAt: string;
    updatedAt: string;
}

//Request kısmı
export interface CreateRequestLogDto

{
    url: string;
    method: string;
    body?: string;
    requestHeaders: RequestHeaderDto[];
    requestParameters: RequestParameterDto[];
}

export interface RequestHeaderDto extends BaseDto

{
    key: string;
    value: string;
}

export interface RequestParameterDto extends BaseDto

{
    key: string;
    value: string;
}

export interface RequestLogDto extends BaseDto

{
    url: string;
    method: string; 
    body?: string; 
    requestHeaders: RequestHeaderDto[];
    requestParameters: RequestParameterDto[];
}

export interface UpdateRequestLogDto extends BaseDto

{
    url: string;
    method: string;
    body?: string;
    requestHeaders: RequestHeaderDto[];
    requestParameters: RequestParameterDto[];
}

//Response Kısmı

export interface ResponseHeaderDto extends BaseDto

{
    key: string;
    value: string;
}

export interface UpdateResponseLogDto extends BaseDto

{
    statusCode: number;
    body?: string;
    executionTimeMs: number;
    responseHeaders: ResponseHeaderDto[];
}

export interface ResponseLogDto extends BaseDto

{
    requestLogId: string;
    statusCode: number;
    body?: string;
    executionTimeMs: number;
    responseHeaders: ResponseHeaderDto[];
}

export interface CreateResponseLogDto {
    requestLogId: string;
    statusCode: number;
    body?: string;
    executionTimeMs: number;
    responseHeaders: ResponseHeaderDto[];
}
