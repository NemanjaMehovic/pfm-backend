import { Split } from "./split";

export class Transaction {
    id?: String;
    beneficiary_name?: String;
    date?: Date;
    direction?: String;
    amount?: number;
    description?: String;
    currency?: String;
    mcc?: String;
    kind?: String;
    catcode?: String;
    splits?: Array<Split>;
}   