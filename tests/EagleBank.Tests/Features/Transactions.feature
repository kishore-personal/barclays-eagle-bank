Feature: Transactions
  An authenticated owner can deposit and fetch a transaction on their account.

  Scenario: Deposit with required data
    Given I am authenticated as a registered user
    And I have a personal bank account
    When I deposit 10.50 into that account
    Then I receive a 201 response
    And the response is a TransactionResponse
    When I fetch the created account
    Then I receive a 200 response
    And the account balance is 10.50
    And the test log sink contains no denylist PII

  Scenario: Deposit with an invalid amount
    Given I am authenticated as a registered user
    And I have a personal bank account
    When I deposit 10.999 into that account
    Then I receive a 400 response

  Scenario: Deposit without authentication
    Given I am not authenticated
    When I deposit 10.50 into account "01234567"
    Then I receive a 401 response

  Scenario: Deposit into another user's account
    Given I am authenticated as a registered user
    And another registered user exists
    And the other user has a personal bank account
    When I deposit 10.50 into the other user's account
    Then I receive a 403 response

  Scenario: Deposit into a missing account
    Given I am authenticated as a registered user
    When I deposit 10.50 into account "01999999"
    Then I receive a 404 response

  Scenario: Fetch own transaction
    Given I am authenticated as a registered user
    And I have a personal bank account
    And I have deposited 10.50 into that account
    When I fetch the created transaction
    Then I receive a 200 response
    And the response is a TransactionResponse

  Scenario: Fetch transaction without authentication
    Given I am not authenticated
    When I fetch transaction "tan-abc123" on account "01234567"
    Then I receive a 401 response

  Scenario: Fetch transaction with a bad transactionId
    Given I am authenticated as a registered user
    And I have a personal bank account
    When I fetch transaction "bad-id" on the created account
    Then I receive a 400 response

  Scenario: Fetch transaction with a bad accountNumber
    Given I am authenticated as a registered user
    When I fetch transaction "tan-abc123" on account "not-an-account"
    Then I receive a 400 response

  Scenario: Fetch transaction on another user's account
    Given I am authenticated as a registered user
    And another registered user exists
    And the other user has a personal bank account
    When I fetch transaction "tan-doesnotexist1" on the other user's account
    Then I receive a 403 response

  Scenario: Fetch unknown transaction
    Given I am authenticated as a registered user
    And I have a personal bank account
    When I fetch transaction "tan-doesnotexist1" on the created account
    Then I receive a 404 response

  Scenario: Fetch transaction that belongs to another account
    Given I am authenticated as a registered user
    And I have a personal bank account
    And I have deposited 5.00 into that account
    And I have a second personal bank account
    When I fetch the created transaction on the second account
    Then I receive a 404 response

  Scenario: Deposit that would exceed the balance cap
    Given I am authenticated as a registered user
    And I have a personal bank account
    And that account has a balance of 10000.00
    When I deposit 0.01 into that account
    Then I receive a 422 response
    When I fetch the created account
    Then I receive a 200 response
    And the account balance is 10000.00
