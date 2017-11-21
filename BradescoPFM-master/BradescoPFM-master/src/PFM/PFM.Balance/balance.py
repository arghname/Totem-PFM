class Entry:
    def __init__(self, name, value, category):
        self.name = name
        self.value = value
        self.category = category

    def serialize(self):
        return {
            'Name': self.name,
            'Value': self.value,
            'Category': self.category,
        }


class Summary:
    def __init__(self, account_id, days, entries):
        self.account_id = account_id
        self.days = days
        self.entries = entries

    def serialize(self):
        return {
            'AccountId': self.account_id,
            'Days': self.days,
            'Entries': self.entries,
        }


class Recommendation:
    def __init__(self, account_id, value, available_month):
        self.account_id = account_id
        self.value = value
        self.available_month = available_month

    def serialize(self):
        return {
            'AccountId': self.account_id,
            'Value': self.value,
            'AvailableMonth': self.available_month,
        }