import numpy
from sklearn.linear_model import SGDRegressor, Ridge, LinearRegression, RANSACRegressor
from sklearn import preprocessing
import matplotlib.pyplot as plt

#def smape(A, F):
#    return 100/len(A) * numpy.sum(2 * numpy.abs(F - A) / (numpy.abs(A) + numpy.abs(F)))

#def _error(actual: numpy.ndarray, predicted: numpy.ndarray):
#    return actual - predicted

#def mse(actual: numpy.ndarray, predicted: numpy.ndarray):
#    return numpy.mean(numpy.square(_error(actual, predicted)))

#def rmse(actual: numpy.ndarray, predicted: numpy.ndarray):
#    return numpy.sqrt(mse(actual, predicted))

#def nrmse(actual: numpy.ndarray, predicted: numpy.ndarray):
#    return rmse(actual, predicted) / (actual.max() - actual.min())

def rmse(y_pred, y_actual):
    return (sum([(y_pred_i - y_actual_i)**2 for y_pred_i, y_actual_i in zip(y_pred, y_actual)])/len(y_actual))**(1/2)

def nrmse(y_pred, y_actual):
    return rmse(y_pred, y_actual)/(max(y_actual) - min(y_actual))

#read from file and normalize

xTraining = []
yTraining = []
xTest = []
yTest = []

with open ("files/1.txt", "r") as datasetFile:
    data = datasetFile.read()
dataStrings = data.split("\n")

attributesCount = int(dataStrings[0])
trainingCount = int(dataStrings[1])

for i in range(2, 2 + trainingCount):
    attributeStrings = dataStrings[i].split(" ")
    attributes = []
    for j in range(0, attributesCount):
        attributes.append(int(attributeStrings[j]))
    xTraining.append(attributes)
    yTraining.append(attributeStrings[attributesCount])

testCount = int(dataStrings[2 + trainingCount])
xTraining = preprocessing.normalize(xTraining)

for i in range(3 + trainingCount, 3 + trainingCount + testCount):
    attributeStrings = dataStrings[i].split(" ")
    attributes = []
    for j in range(0, attributesCount):
        attributes.append(int(attributeStrings[j]))
    xTest.append(attributes)
    yTest.append(attributeStrings[attributesCount])

xTest = preprocessing.normalize(xTest)
yTraining, yTest = numpy.asarray(yTraining, dtype=numpy.float), numpy.array(yTest, dtype=numpy.float)
xTraining = numpy.hstack((xTraining, numpy.ones((xTraining.shape[0], 1))))
xTest = numpy.hstack((xTest, numpy.ones((xTest.shape[0], 1))))

#mnk with svd

modelRidge = Ridge(alpha=0.5, solver='svd')
modelRidge.fit(xTraining, yTraining)
yPredictedTrain = modelRidge.predict(xTraining)
errorTrain = nrmse(yPredictedTrain, yTraining)
print(errorTrain)
yPredictedTest = modelRidge.predict(xTest)
errorTest = nrmse(yPredictedTest, yTest)
print(errorTest)

#sgd

#sgdRegressor = SGDRegressor(shuffle=True, max_iter=1000000, tol=1e-20, penalty="elasticnet", alpha=0.1, learning_rate="optimal", l1_ratio=0.4, n_iter_no_change=100)
#sgdModel = sgdRegressor.fit(xTraining, yTraining)
#yPredicted = sgdModel.predict(xTest)
#sgdNrmse = nrmse(numpy.asarray(yTest, dtype=numpy.int64), numpy.asarray(yPredicted, dtype=numpy.int64))
#sgdSmape = smape(numpy.asarray(yTest, dtype=numpy.int64), numpy.asarray(yPredicted, dtype=numpy.int64))
#print(sgdNrmse)
#print(sgdSmape)