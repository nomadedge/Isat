import os
import re
import matplotlib.pyplot as pyplot
from sklearn.naive_bayes import MultinomialNB
from sklearn.metrics import roc_curve
from sklearn.metrics import confusion_matrix
from sklearn.feature_extraction.text import CountVectorizer
from sklearn.model_selection import cross_val_score

messages, isLegit = [], []
messageFolds, isLegitFolds = [], []
for i in range(1, 11):
    m = []
    isl = []
    for dirpath, dirnames, filenames in os.walk('messages/part' + str(i)):
        for filename in filenames:
            buffer = []
            with open(dirpath + '/' + filename, 'r') as file:
                for line in file:
                    buffer.extend(re.findall(r'\d+', line))
            m.append(' '.join(buffer))
            messages.append(' '.join(buffer))
            if filename.find('legit') > -1:
                isLegit.append(1)
                isl.append(1)
            else:
                isLegit.append(0)
                isl.append(0)
    messageFolds.append(m)
    isLegitFolds.append(isl)

classifier = MultinomialNB(fit_prior=False)
vectorizer = CountVectorizer(ngram_range=(1, 30))
ngramCounts = vectorizer.fit_transform(messages)
classifier.fit(ngramCounts, isLegit)
#falsePositiveRate, truePositiveRate, threshold = roc_curve(isLegit, classifier.predict_proba(ngramCounts)[:, 1], pos_label = 1)

#pyplot.title('ROC')
#pyplot.plot(falsePositiveRate, truePositiveRate)
#pyplot.xlim([-0.05, 1.0])
#pyplot.ylim([0.0, 1.05])
#pyplot.ylabel('True Positive Rate')
#pyplot.xlabel('False Positive Rate')
#pyplot.show()

#w, h = 2, 2;
#confusionMatrix = [[0 for x in range(w)] for y in range(h)] 

for i in range(0, 10):
    ngrams = ngramCounts[109 * i : 109 * (i + 1)]
    isLegitPred = classifier.predict(ngrams)

    print("fold " + str(i))
    print(confusion_matrix(isLegitFolds[i], isLegitPred))
    print()

print("all folds")
print(confusion_matrix(isLegit, classifier.predict(ngramCounts)))

#spamProbabilities, accuracies = [], []
#vectorizer = CountVectorizer(ngram_range=(1, 5))
#ngramCounts = vectorizer.fit_transform(messages)
#iterationsCount = 100
#for j in range(1, iterationsCount, 1):
#    classifier = MultinomialNB(class_prior=[j/iterationsCount, 1-j/iterationsCount])
#    scores = cross_val_score(classifier, ngramCounts, isLegit, cv=10)
#    spamProbabilities.append(j/iterationsCount)
#    accuracies.append(scores.mean())

#pyplot.plot(spamProbabilities, accuracies, label="Accuracy - spam probability dependency")
#pyplot.xlabel("Spam probability")
#pyplot.ylabel("Accuracy")
#pyplot.legend()
#pyplot.show()