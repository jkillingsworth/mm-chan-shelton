## Overview

This is a study of [*An Electronic Market-Maker*](http://dspace.mit.edu/handle/1721.1/7220) by Nicholas Tung Chan and Christian Shelton.

#### 01 - Expected Profit

This is a Monte Carlo simulation of the basic model that computes the expected profit. The following is a reproduction of Figure 3 from the paper:

![ExpectedProfit-Result](images/ExpectedProfit-Result.png)

These are plots of the true price and market price for strategies 1, 2, and 3, respectively, when the noise factor alphaU = 0.4:

![ExpectedProfit-Prices-alphaU-0.4-imbalance-1](images/ExpectedProfit-Prices-alphaU-0.4-imbalance-1.png)
![ExpectedProfit-Prices-alphaU-0.4-imbalance-2](images/ExpectedProfit-Prices-alphaU-0.4-imbalance-2.png)
![ExpectedProfit-Prices-alphaU-0.4-imbalance-3](images/ExpectedProfit-Prices-alphaU-0.4-imbalance-3.png)

These are plots of the true price and market price for strategies 1, 2, and 3, respectively, when the noise factor alphaU = 1.0:

![ExpectedProfit-Prices-alphaU-1.0-imbalance-1](images/ExpectedProfit-Prices-alphaU-1.0-imbalance-1.png)
![ExpectedProfit-Prices-alphaU-1.0-imbalance-2](images/ExpectedProfit-Prices-alphaU-1.0-imbalance-2.png)
![ExpectedProfit-Prices-alphaU-1.0-imbalance-3](images/ExpectedProfit-Prices-alphaU-1.0-imbalance-3.png)

These are plots of the true price and market price for strategies 1, 2, and 3, respectively, when the noise factor alphaU = 1.6:

![ExpectedProfit-Prices-alphaU-1.6-imbalance-1](images/ExpectedProfit-Prices-alphaU-1.6-imbalance-1.png)
![ExpectedProfit-Prices-alphaU-1.6-imbalance-2](images/ExpectedProfit-Prices-alphaU-1.6-imbalance-2.png)
![ExpectedProfit-Prices-alphaU-1.6-imbalance-3](images/ExpectedProfit-Prices-alphaU-1.6-imbalance-3.png)

#### 02 - Basic Model (SARSA)

This is an implementation of the basic model that uses the SARSA learning method to choose the optimum strategy. The following is a reproduction of Figure 5 from the paper:

* Episode 25

![BasicModel-Prices-025](images/BasicModel-Prices-025.png)

* Episode 100

![BasicModel-Prices-100](images/BasicModel-Prices-100.png)

* Episode 200

![BasicModel-Prices-200](images/BasicModel-Prices-200.png)

* Episode 500

![BasicModel-Prices-500](images/BasicModel-Prices-500.png)

The following is a reproduction of Figure 6a from the paper:

![BasicModel-EndOfEpisodeProfitSum](images/BasicModel-EndOfEpisodeProfitSum.png)

The following is a reproduction of Figure 6b from the paper:

![BasicModel-EndOfEpisodeInventory](images/BasicModel-EndOfEpisodeInventory.png)

The following is a reproduction of Figure 6c from the paper:

![BasicModel-AveragePriceDeviation](images/BasicModel-AveragePriceDeviation.png)

#### 03 Extended Model (SARSA)

This is an implementation of the extended model that uses the SARSA learning method to choose the optimum spread. The following is a reproduction of Figure 9 from the paper:

* Episode 25

![ExtendedModel-Prices-025](images/ExtendedModel-Prices-025.png)

* Episode 100

![ExtendedModel-Prices-100](images/ExtendedModel-Prices-100.png)

* Episode 200

![ExtendedModel-Prices-200](images/ExtendedModel-Prices-200.png)

* Episode 500

![ExtendedModel-Prices-500](images/ExtendedModel-Prices-500.png)

The following is a reproduction of Figure 10a from the paper:

![ExtendedModel-EndOfEpisodeProfitSum](images/ExtendedModel-EndOfEpisodeProfitSum.png)

The following is a reproduction of Figure 10b from the paper:

![ExtendedModel-AverageEpisodicSpread](images/ExtendedModel-AverageEpisodicSpread.png)

The following is a reproduction of Figure 10c from the paper:

![ExtendedModel-AveragePriceDeviation](images/ExtendedModel-AveragePriceDeviation.png)

The following is a reproduction of Figure 10d from the paper:

![ExtendedModel-EndOfEpisodeInventory](images/ExtendedModel-EndOfEpisodeInventory.png)
