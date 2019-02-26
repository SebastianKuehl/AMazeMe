# import and prepare data #
#df <-read.csv2(file="AMaze_Me_HMD.csv", sep=";")
#df2 <-read.csv2(file="AMaze_Me_NON_HMD.csv", sep=";")
#df3 <-read.csv2(file="Sömmad_HMD.csv", sep=";")
#df4 <-read.csv2(file="Sömmad_NON_HMD.csv", sep=";")

#df$game <- "AMaze Me"
#df$condition <- "HMD"
#df2$game <- "AMaze Me"
#df2$condition <- "NonHMD"
#df3$game <- "Sömmad"
#df3$condition <- "HMD"
#df4$game <- "Sömmad"
#df4$condition <- "NonHMD"

#df_all <- rbind(df,df2,df3,df4)
#df_all$game <- factor(df_all$game)
#df_all$condition <- factor(df_all$condition)
#df_all$participant <- rep(seq(1:13),4)

#save(file="UX_All.RData", df_all)

# load data #
load("UX_All.RData")
#summary(df_all)

# plot #
with(df_all, boxplot(X8~game, notch=T))

# test #
wilcox.test(X8 ~ game, data = df_all[df_all$condition=="HMD",], paired=T)
wilcox.test(X1 ~ game, data = df_all[df_all$condition=="NonHMD",], paired=T)

# Median + IQR #
UX.ag <- aggregate(. ~ game+condition, df_all, function(x) c(Median = median(x), IQR = IQR(x)))

library(xtable)
xtable(do.call(data.frame, UX.ag))
View(xtable(do.call(data.frame, UX.ag)))
