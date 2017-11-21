# -*- coding: cp1252 -*-
from datetime import date
from flask import Flask, jsonify, request, send_file
from balance import Entry, Summary, Recommendation

app = Flask(__name__)
# Aqui vai o algoritmo do Scikitlearn

# GET /
@app.route("/")
def hello():
    return "Bem vindo ao PFM core API."

# GET /api/balance/1001890
@app.route("/api/balance/<account_id>")
def getBalance(account_id):
    return jsonify(AccountId=account_id, Value=1000.00)

# GET /api/summary/past/1001890?days=7
@app.route("/api/summary/past/<account_id>")
def getPastSummary(account_id):
    days = request.args.get('days')
    # Process data
    entries = []
    entries.append(Entry('Salario', 1500, 'In'))
    entries.append(Entry('Restaurante', 300, 'Out'))
    entries.append(Entry('Supermercado', 156, 'Out'))
    Entries=[e.serialize() for e in entries]
    summary = Summary(account_id, days, Entries)
    return jsonify(summary.serialize())

# GET /api/summary/future/1001890?days=7
@app.route("/api/summary/future/<account_id>")
def getFutureSummary(account_id):
    days = request.args.get('days')
    # Process data
    entries = []
    entries.append(Entry('Salario', 4000, 'In'))
    entries.append(Entry('Restaurante', 123, 'Out'))
    Entries=[e.serialize() for e in entries]
    summary = Summary(account_id, days, Entries)
    return jsonify(summary.serialize())

# GET /api/summary/chart/1001890?days=7
@app.route('/api/summary/chart/<account_id>')
def getChart(account_id):
    days = request.args.get('days')
    filename = 'grafico.png'
    return send_file(filename, mimetype='image/png')

# GET /api/predict/purchase/1001890?value=1500
@app.route("/api/predict/purchase/<account_id>")
def getAvailablMonthForPurchase(account_id):
    value = float(request.args.get('value'))

    # TODO: Get savings

    savings = [200, 150, 178]
    avg = average(savings)

    predicted=predict(1000.0, avg, 5)
    month=get_month_for_value(predicted,value)

    recommendation=Recommendation(account_id, value, month)

    return jsonify(recommendation.serialize())


def average(savings):
    return sum(savings) / float(len(savings))

def predict(current, average, months):
    accumulated = []
    for i in range(months):
        accumulated.append(current)
        current+=average
    return accumulated

def get_month_for_value(predicted, value):
    months = len(predicted)
    for i in range(months):
        if(predicted[i]>=value):
            return i
    return -1