export interface Progression {
    date: string;
    percent: number;
  }
  
  export interface TodoItem {
    id: number;
    title: string;
    description: string;
    category: string;
    isCompleted: boolean;
    progressions: Progression[];
  }
  
  export interface ApiResponse<T> {
    data: T;
    status: number;
    statusText: string;
  }