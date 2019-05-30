export interface ApiResponse<T = null> {
    data?: T | T[];
    error: string;
    success: boolean;
}
