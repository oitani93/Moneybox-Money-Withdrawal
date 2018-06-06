# Moneybox-Money-Withdrawal

I have attempted to  make this as much domain driven as possible.

The article I read was interesting and helpful, and helped me learn more about domain driven models.

Therefore, I have now removed the accountService that I originally implemented, and put all the logic in the domain model.

I have also attempted to create a domain event for when balance is equal to 0 and successfully created a test case for it.

I would appreciate to get more feedback from you.

Many thanks.

---------------------------------------------------------------
MY CHANGES:

- I created a new service(AccountService) in the services folder under the domain folder.

- The service will be called from the following classes, WithdrawMoney and TransferMoney.

- The service will take care of updating properties(balance, withdrawn, paidIn) of an Account if validation passes.

- The service will also handle validation. 
***All validation logic (i.e to decide whether an exception should be thrown) are retrieved from the Account class***

- Once the relevant method in the service gets called, the TransferMoney / WithdrawMoney classes in the Features folder
will call the Update method from the accountRepository. On success, a check is placed by calling the logic from the accounts
class to decide whether a notification email should be sent.
***I have slightly changed the order of behaviour for sending notification emails, I believe the notification email should be sent 
after the update method is executed successfully***

- Also I have amended the implementation for updating withdrawn, I believe it makes more sense if '+' used rather than '-'

- I would also like to point out the following:

-> If the current logic in the domain models did become more complex I would ideally place them in 
the accountService and let the service handle the business logic.
