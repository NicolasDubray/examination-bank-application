# Examination - Bank Application

**Elev:** Nicolas Dubray
**Datum:** 2026-04-17
**Domän:** Bank

## Instruktioner

Du har fått en Console-applikation i **Onion Architecture** som innehåller buggar och
saknade metodimplementationer. Din uppgift är att:

1. **Felsöka och fixa 8 buggar** i Application- och Infrastructure-lagren
2. **Implementera 4 saknade metoder** (markerade med `NotImplementedException`)

## Regler

- Du får **INTE** ändra namn på klasser, metoder, namespaces eller interfaces
- Du får **INTE** lägga till nya projekt eller ändra projektstrukturen
- Du får **INTE** ändra i testprojektet
- Alla metodsignaturer måste vara exakt som de är (parametrar, returtyper)
- Följ Onion Architecture-principerna (beroenden pekar inåt)

## Projektstruktur

```
BankApp/
├── Bank.Domain/       # Entiteter, enums, interfaces (ÄNDRA INTE)
├── Bank.Application/  # Services - HÄR FIXAR DU BUGGAR & IMPLEMENTERAR METODER
├── Bank.Infrastructure/ # Repositories & factories - KAN INNEHÅLLA BUGGAR
└── Bank.Console/      # Consoleappen (valfritt att ändra för felsökning)

BankApp.Tests/          # Testprojektet - ÄNDRA INTE
```

## Att fixa: Buggar

1. **Bugg i `Deposit`** - Deposit adds the amount twice to balance
2. **Bugg i `GetAccountSummary`** - GetAccountSummary swaps interest rate and monthly fee values
3. **Bugg i `TransferWithFee`** - TransferWithFee credits receiver with amount PLUS fee instead of just amount
4. **Bugg i `GetByAccountNumber`** [Infrastructure] - AccountRepository GetByAccountNumber always returns null (wrong comparison)
5. **Bugg i `Transfer`** - Transfer subtracts from the wrong account (subtracts from destination)
6. **Bugg i `GetMonthlyFee`** - GetMonthlyFee always returns 0 regardless of account type
7. **Bugg i `GetHighValueTransactions`** - GetHighValueTransactions ignores the date range filter entirely
8. **Bugg i `CalculateCompoundInterest`** - CalculateCompoundInterest uses simple interest formula instead of compound (Math.Pow replaced)

## Att implementera: Saknade metoder

1. **`GetWithdrawalLimit`** - Implement GetWithdrawalLimit that returns the withdrawal limit for an account type using the factory
2. **`Transfer`** - Implement Transfer method that moves money between two accounts
3. **`CalculateCompoundInterest`** - Implement CalculateCompoundInterest that calculates the compound interest earned using the account rule's annual rate over a number of months
4. **`TransferWithFee`** - Implement TransferWithFee that transfers amount plus a percentage fee from one account to another and records two transactions

## Verifiering

När du är klar, kör testerna:

```bash
dotnet test BankApp.Tests/Bank.Tests.csproj
```

**Alla tester gröna = Godkänd uppgift!**
