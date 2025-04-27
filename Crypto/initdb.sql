USE CryptoDb_HGH7VW;
GO


INSERT INTO Cryptocurrencies (Name, Symbol, CurrentPrice) VALUES
('Bitcoin', 'BTC', 60000.00),
('Ethereum', 'ETH', 2500.00),
('Ripple', 'XRP', 1.50),
('Litecoin', 'LTC', 180.00),
('Cardano', 'ADA', 1.20),
('Polkadot', 'DOT', 20.00),
('Binance Coin', 'BNB', 500.00),
('Chainlink', 'LINK', 25.00),
('Stellar', 'XLM', 0.30),
('Dogecoin', 'DOGE', 0.15),
('Solana', 'SOL', 150.00),
('Avalanche', 'AVAX', 50.00),
('Cosmos', 'ATOM', 15.00),
('Algorand', 'ALGO', 0.90),
('Tezos', 'XTZ', 3.00);
GO

INSERT INTO Users (Username, Email, Password) VALUES
('testUser1', 'user1@example.com', 'password123'),
('testUser2', 'user2@example.com', 'password123'),
('testUser3', 'test.user3@example.com', 'password123'),
('testUser4', 'test.user4@example.com', 'password123');
GO

INSERT INTO Wallets (UserId, Balance) VALUES
(1, 1000.00), 
(2, 1000.00), 
(3, 5000.00), 
(4, 0.00);    
GO

INSERT INTO PortfolioItems (WalletId, CryptocurrencyId, Quantity, PurchasePrice) VALUES
(1, 1, 0.1, 59000.00),  -- Bitcoin, 0.1 
(1, 2, 2.0, 2400.00),   -- Ethereum, 2.0 
(2, 3, 100.0, 1.40),    -- Ripple, 100
(3, 1, 0.05, 58000.00), -- Bitcoin, 0.05
(3, 2, 1.5, 2300.00),   -- Ethereum, 1.5
(3, 10, 100.0, 0.10);   -- Dogecoin, 100 
GO

INSERT INTO Transactions (UserId, CryptocurrencyId, Type, Quantity, Price, Timestamp) VALUES
(1, 1, 'Buy', 0.1, 59000.00, '2025-04-21 10:00:00'),
(1, 2, 'Buy', 2.0, 2400.00, '2025-04-21 10:01:00'),
(1, 3, 'Sell', 25.0, 1.60, '2025-04-21 10:03:00'),
(2, 1, 'Buy', 0.01, 61000.00, '2025-04-21 10:04:00'),
(3, 1, 'Buy', 0.05, 58000.00, '2025-04-21 10:05:00'),
(3, 2, 'Buy', 1.5, 2300.00, '2025-04-21 10:06:00'),
(3, 10, 'Buy', 100.0, 0.10, '2025-04-21 10:07:00');
GO

INSERT INTO PriceHistories (CryptocurrencyId, Price, Timestamp) VALUES
(1, 60000.00, '2025-04-21 10:00:00'),
(2, 2500.00, '2025-04-21 10:00:00'),
(1, 61000.00, '2025-04-21 10:30:00'),
(2, 2600.00, '2025-04-21 10:30:00'),
(3, 1.60, '2025-04-21 10:30:00'),
(10, 0.10, '2025-04-21 10:00:00'),
(10, 0.15, '2025-04-21 10:30:00');
GO

