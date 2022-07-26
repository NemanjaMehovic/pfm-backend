import { Transaction } from "./transaction";

export class TransactionsList {
    page_size?: number;
    page?: number;
    total_count?: number;
    sort_by?: String;
    sort_order?: String;
    items?: Array<Transaction>;
}